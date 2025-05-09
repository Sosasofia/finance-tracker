using AutoMapper;
using FinanceTracker.Server.Models;
using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.Entities;
using FinanceTracker.Server.Models.Response;
using FinanceTracker.Server.Repositories;

namespace FinanceTracker.Server.Services
{
    public class TransactionService
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
            var transaction = _mapper.Map<Transaction>(transactionCreateDTO);
            transaction.UserId = userID;

            // Add installment logic
            if (transactionCreateDTO.IsCreditCardPurchase)
            {
                var installments = GenerateInstallments(transactionCreateDTO);

                if (installments == null || !installments.Any())
                {
                    return new Response<TransactionResponse>("At least one installment must be provided for a credit card purchase.");
                }

                transaction.InstallmentsList = installments.ToList();
            }

            // Add reimbursement logic
            if (transactionCreateDTO.IsReimbursement)
            {
                if (transactionCreateDTO.Reimbursement == null)
                {
                    return new Response<TransactionResponse>("Reimbursement details must be provided if transaction is marked as reimbursement.");
                }

                var reimbursement = GenerateReimbursement(transactionCreateDTO);

                transaction.Reimbursement = reimbursement;
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

        // Function to generate reimbursement records
        private Reimbursement GenerateReimbursement(TransactionCreateDTO transaction)
        {
            var reimbursement = new Reimbursement
            {
                Amount = transaction.Reimbursement.Amount,
                Date = transaction.Reimbursement.Date,
                Reason = transaction.Reimbursement.Reason,
            };

            return reimbursement;
        }
    }
}
