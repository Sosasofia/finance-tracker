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
        public async Task<Response<TransactionResponse>> AddTransactionAsync(TransactionCreateDTO transactionCreateDTO)
        {
            if (transactionCreateDTO == null)
                return new Response<TransactionResponse>("Transaction can not be null");

            var transaction = _mapper.Map<Transaction>(transactionCreateDTO);

            // Add installment logic
            if (transactionCreateDTO.IsCreditCardPurchase)
            {
                var installments = GenerateInstallments(transactionCreateDTO);

                if (installments == null || !installments.Any())
                    return new Response<TransactionResponse>("Installments can not be null");

                transaction.InstallmentsList = installments.ToList();
            }

            // Add reimbursment logic
            if (transactionCreateDTO.IsReimbursment && transactionCreateDTO.ReimburstmentDTO != null)
            {
                var reimburstment = GenerateReimbursment(transactionCreateDTO);

                transaction.Reimburstment = reimburstment;
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

        public async Task DeleteTransactionAsync(Guid id)
        {
            await _transactionRepository.DeleteTransactionAsync(id);
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

        // Function to generate reimburstment records
        private Reimburstment GenerateReimbursment(TransactionCreateDTO transaction)
        {
            var reimburstment = new Reimburstment
            {
                Amount = transaction.ReimburstmentDTO.Amount,
                Date = transaction.ReimburstmentDTO.Date,
                Reason = transaction.ReimburstmentDTO.Reason,
            };

            return reimburstment;
        }
    }
}
