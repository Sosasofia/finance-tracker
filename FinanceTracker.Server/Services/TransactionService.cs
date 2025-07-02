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
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
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
