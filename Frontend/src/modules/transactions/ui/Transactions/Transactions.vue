<script lang="ts" setup>
import TransactionList from '@/modules/transactions/ui/TransactionList/TransactionList.vue'
import { useTransactions } from '@/modules/transactions/ui/Transactions/useCases'

const { transactions } = useTransactions()
</script>

<template>
  <TransactionList :transactions="transactions" />
</template>
