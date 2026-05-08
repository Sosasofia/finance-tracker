import { computed, inject } from '@angular/core';
import {
  patchState,
  signalStore,
  withComputed,
  withHooks,
  withMethods,
  withState,
} from '@ngrx/signals';
import { firstValueFrom } from 'rxjs';
import { TransactionService } from '../../../core/services/transaction.service';
import { Transaction } from '../../../shared/models/transaction.model';

interface TransactionState {
  transactions: Transaction[];
  isLoading: boolean;
  error: string | null;
}

const initialState: TransactionState = {
  transactions: [],
  isLoading: false,
  error: null,
};

export const TransactionStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withComputed((store) => ({
    totalIncome: computed(() =>
      store
        .transactions()
        .filter((t) => t.type === 'Income')
        .reduce((sum, t) => sum + t.amount, 0)
    ),
    totalExpense: computed(() =>
      store
        .transactions()
        .filter((t) => t.type === 'Expense')
        .reduce((sum, t) => sum + t.amount, 0)
    ),
  })),

  withMethods((store, transactionService = inject(TransactionService)) => ({
    async loadTransactions() {
      patchState(store, { isLoading: true, error: null });

      try {
        const transactions = await firstValueFrom(transactionService.getTransactions());
        patchState(store, { transactions, isLoading: false });
      } catch {
        patchState(store, { isLoading: false, error: 'Failed to load transactions' });
      }
    },

    async deleteTransaction(transactionId: string) {
      patchState(store, { isLoading: true, error: null });

      try {
        await firstValueFrom(transactionService.deleteTransaction(transactionId));

        patchState(store, (state) => ({
          transactions: state.transactions.filter((t) => String(t.id) !== String(transactionId)),
          isLoading: false,
        }));
      } catch (error) {
        patchState(store, {
          isLoading: false,
          error: 'Failed to delete the transaction.',
        });

        throw error;
      }
    },

    addTransactionLocal(newTx: Transaction) {
      patchState(store, (state) => ({
        transactions: [...state.transactions, newTx],
      }));
    },
  })),

  withHooks({
    onInit(store) {
      store.loadTransactions();
    },
  })
);
