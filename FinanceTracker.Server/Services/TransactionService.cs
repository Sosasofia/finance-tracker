using AutoMapper;
using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.DTOs.Response;
using FinanceTracker.Server.Models.Entities;
using FinanceTracker.Server.Repositories;

namespace FinanceTracker.Server.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _catalogRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IMapper _mapper;

        public TransactionService(
            ITransactionRepository transactionRepository, 
            IMapper mapper, 
            ICategoryRepository catalogRepository, 
            IPaymentMethodRepository paymentMethodRepository)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _catalogRepository = catalogRepository;
            _paymentMethodRepository = paymentMethodRepository;
        }

        /// <summary>
        /// Adds a transaction to the database.
        /// </summary>
        public async Task<Response<TransactionResponse>> AddTransactionAsync(TransactionCreateDTO transactionCreateDTO, Guid userID)
        {

            if (!transactionCreateDTO.IsReimbursement)
            {
                transactionCreateDTO.Reimbursement = null;
            }
            if (!transactionCreateDTO.IsCreditCardPurchase)
            {
                transactionCreateDTO.Installment = null;
            }

            var validationErrors = transactionCreateDTO.Validate();
            if (validationErrors.Any())
            {
                return new Response<TransactionResponse>(string.Join(" ", validationErrors));
            }


            if (transactionCreateDTO.CategoryId.HasValue)
            {
                var categoryExists = await _catalogRepository.CategoryExistsAsync(transactionCreateDTO.CategoryId.Value);
                if (!categoryExists)
                {
                    return new Response<TransactionResponse>("The specified category does not exist.");
                }
            }

            if (transactionCreateDTO.PaymentMethodId.HasValue)
            {
                var paymentMethodExists = await _paymentMethodRepository.PaymentMethodExistsAsync(transactionCreateDTO.PaymentMethodId.Value);
                if (!paymentMethodExists)
                {
                    return new Response<TransactionResponse>("The specified payment method does not exist.");
                }
            }

            var transaction = _mapper.Map<Transaction>(transactionCreateDTO);
            transaction.UserId = userID;

            if (transactionCreateDTO.IsCreditCardPurchase)
            {
                var installments = GenerateInstallments(transactionCreateDTO);

                if (installments == null || !installments.Any())
                {
                    return new Response<TransactionResponse>("Failed to generate installments. Ensure valid credit card purchase details are provided.");
                }

                transaction.InstallmentsList = installments.ToList();
            }

            var result = await _transactionRepository.AddTransactionAsync(transaction);

            return new Response<TransactionResponse>(_mapper.Map<TransactionResponse>(result));
        }


        /// <summary>
        /// Gets all transactions for a user by id.
        /// </summary>
        public async Task<IEnumerable<TransactionResponse>> GetTransactionsByUserAsync(Guid userId)
        {
            var transactions = await _transactionRepository.GetTransactionsByUserAsync(userId);

            var res = _mapper.Map<IEnumerable<Transaction>, IEnumerable<TransactionResponse>>(transactions);

            return res;
        }

        public async Task<Response<bool>> DeleteTransactionAsync(Guid transactionId, Guid userId)
        {
            var transaction = await _transactionRepository.GetTransactionsByIdAndUserAsync(transactionId, userId);

            if (transaction == null)
            {
                throw new UnauthorizedAccessException("Denied access. Transaction not found or does not belong to the user.");
            }

            await _transactionRepository.DeleteTransactionAsync(transaction);

            return new Response<bool>("Transaction deleted.");
        }

        public async Task<TransactionResponse> GetTransactionByIdAndUserAsync(Guid transactionId, Guid userId)
        {
            var transaction = await _transactionRepository.GetTransactionsByIdAndUserAsync(transactionId, userId);

            if (transaction == null)
            {
                throw new UnauthorizedAccessException("Transaction not found or denied access.");
            }

            return _mapper.Map<TransactionResponse>(transaction);
        }

        public async Task<TransactionResponse> RestoreDeleteTransactionAsync(Guid transactionId, Guid userId)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAndUserIncludingDeletedAsync(transactionId, userId);

            if (transaction == null)
            {
                throw new UnauthorizedAccessException("Transaction not found or denied access.");
            }
            if (!transaction.IsDeleted)
            {
                throw new UnauthorizedAccessException("Transaction is not deleted.");
            }

            var restoredTransaction = await _transactionRepository.RestoreDeleteTransactionAsync(transaction);

            return _mapper.Map<TransactionResponse>(transaction);
        }

        public async Task<Response<TransactionResponse>> UpdateTransactionAsync(Guid transactionId, TransactionUpdateDTO transactionUpdateDTO, Guid userId)
        {
            var transaction = await _transactionRepository.GetTransactionsByIdAndUserAsync(transactionId, userId);

            var validationErrors = transactionUpdateDTO.Validate();

            if (validationErrors.Any())
            {
                return new Response<TransactionResponse>(string.Join(" ", validationErrors));
            }

            if (transaction == null)
            {
                throw new UnauthorizedAccessException("Transaction not found or denied access.");
            }

            _mapper.Map(transactionUpdateDTO, transaction);

            transaction.LastModifiedAt = DateTime.UtcNow;

            await _transactionRepository.UpdateTransactionAsync(transaction);

            return new Response<TransactionResponse>(_mapper.Map<TransactionResponse>(transaction));
        }

        // Function to generate installments records
        private IEnumerable<Installment> GenerateInstallments(TransactionCreateDTO transaction)
        {
            var installments = new List<Installment>();

            if (transaction.IsCreditCardPurchase && transaction.Installment != null)
            {
                for (int i = 1; i <= transaction.Installment.Number; i++)
                {
                    installments.Add(new Installment
                    {
                        Amount = transaction.Amount / transaction.Installment.Number,
                        InstallmentNumber = i,
                        DueDate = transaction.Date.AddMonths(i),
                        IsPaid = false,
                    });
                }
            }

            return installments;
        }
    }
}
