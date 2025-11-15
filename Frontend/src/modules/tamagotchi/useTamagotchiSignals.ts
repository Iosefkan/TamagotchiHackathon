import { computed } from 'vue'
import { useFinanceSummary } from '@/modules/dashboard/useFinanceSummary'
import {
  TransactionStatus,
  TransactionType,
  type Transaction,
} from '@/modules/transactions/domain/Transaction'

type ReactionTone = 'celebrate' | 'nourish' | 'focus' | 'concern' | 'calm'

interface TamagotchiProgressBar {
  id: 'finance' | 'nutrition'
  label: string
  value: number
  tone: 'positive' | 'warning' | 'critical'
  description: string
}

interface SeasonalMutationState {
  season: string
  ready: boolean
  focus: string
  aura: string
  summary: string
  requirement: string
}

interface TamagotchiStage {
  name: string
  tier: 'sprout' | 'scout' | 'guardian' | 'cocoon'
  descriptor: string
  moodLabel: string
  velocityLabel: string
  stability: number
}

interface TamagotchiMood {
  label: string
  aura: string
  intensity: number
}

interface ReactionTrendEntry {
  id: string
  tone: ReactionTone
  color: string
}

interface TransactionReaction {
  tone: ReactionTone
  color: string
  message: string
  intensity: number
  isNutrition: boolean
}

interface AchievementBadge {
  id: string
  name: string
  rarity: 'common' | 'rare' | 'epic' | 'mythic'
  tags: string[]
  progress: number
  description: string
  limited?: boolean
  timeRemaining?: string
}

interface MerchItem {
  id: string
  name: string
  rarity: 'common' | 'rare' | 'epic'
  slot: 'aura' | 'reaction' | 'quiz'
  effect: string
  copies: number
  duplicates: number
  fusionReady: boolean
  perk: string
  exchangeCost: number
}

interface MiniGame {
  id: string
  title: string
  type: 'drag' | 'quiz' | 'tempo'
  description: string
  reward: string
  status: 'available' | 'cooldown'
  boostTarget: 'finance' | 'nutrition' | 'mood'
}

interface QuizCard {
  id: string
  title: string
  topic: string
  reward: string
  boostTarget: 'finance' | 'nutrition'
  lockingCategory: string
}

interface NutritionPrompt {
  goal: string
  status: string
  action: string
  highlight: string
  onTrack: boolean
}

interface HeaderBadges {
  achievements: number
  merch: number
}

const clamp = (value: number) => Math.max(0, Math.min(100, Math.round(value)))

const NUTRITION_KEYWORDS = [
  'grocery',
  'groceries',
  'market',
  'organic',
  'health',
  'wellness',
  'pharmacy',
  'supplement',
  'fitness',
  'farm',
  'restaurant',
  'cafe',
  'meal',
]

const IMPULSE_KEYWORDS = ['gaming', 'tickets', 'fashion', 'luxury', 'gadget', 'delivery', 'ride']

const WELLNESS_KEYWORDS = ['spa', 'yoga', 'therapy', 'coach']

const TONE_COLORS: Record<ReactionTone, string> = {
  celebrate: '#38d996',
  nourish: '#2bb7ff',
  focus: '#ffb347',
  concern: '#ff7a7a',
  calm: '#978bff',
}

const SEASONS = [
  { months: [11, 0, 1], season: 'Frost Pulse', aura: 'linear-gradient(135deg, #6dd5ed, #2193b0)', focus: 'resilience' },
  { months: [2, 3, 4], season: 'Bloom Circuit', aura: 'linear-gradient(135deg, #fce38a, #f38181)', focus: 'growth' },
  { months: [5, 6, 7], season: 'Solar Drift', aura: 'linear-gradient(135deg, #f6d365, #fda085)', focus: 'momentum' },
  { months: [8, 9, 10], season: 'Harvest Wave', aura: 'linear-gradient(135deg, #d4fc79, #96e6a1)', focus: 'balance' },
]

const toDayKey = (value: string) => {
  const parsed = new Date(value)
  if (Number.isNaN(parsed.getTime())) return null
  return parsed.toISOString().slice(0, 10)
}

const formatShortAmount = (value: number) => {
  if (value >= 1_000_000) return `${(value / 1_000_000).toFixed(1)}M`
  if (value >= 1_000) return `${(value / 1_000).toFixed(1)}K`
  return value.toFixed(0)
}

const analyzeTransaction = (transaction: Transaction) => {
  const normalized = `${transaction.category} ${transaction.name}`.toLowerCase()
  const nutritionKeyword = NUTRITION_KEYWORDS.find((keyword) => normalized.includes(keyword))
  const impulseKeyword = IMPULSE_KEYWORDS.find((keyword) => normalized.includes(keyword))
  const wellnessKeyword = WELLNESS_KEYWORDS.find((keyword) => normalized.includes(keyword))

  return {
    isNutrition: Boolean(nutritionKeyword),
    nutritionKeyword: nutritionKeyword ?? null,
    isImpulse: Boolean(impulseKeyword && transaction.type === TransactionType.Expense),
    isWellness: Boolean(wellnessKeyword),
  }
}

export const useTamagotchiSignals = () => {
  const { transactions } = useFinanceSummary()

  const normalizedTransactions = computed(() => transactions.value ?? [])

  const totals = computed(() => {
    const stats = {
      income: 0,
      expenses: 0,
      nutrition: 0,
      nutritionFamilies: 0,
      impulseSpending: 0,
      impulseTransactions: 0,
      weekTransactions: 0,
      dailySnapshots: [] as Array<{ date: string; income: number; expenses: number; nutrition: number; count: number }>,
      transactionCount: normalizedTransactions.value.length,
    }

    const nutritionFamilies = new Set<string>()
    const dayBuckets = new Map<string, { income: number; expenses: number; nutrition: number; count: number }>()
    const now = Date.now()
    const MS_IN_DAY = 86_400_000
    const weekThreshold = now - MS_IN_DAY * 7

    normalizedTransactions.value.forEach((transaction) => {
      const amount = Math.abs(transaction.amount)
      if (transaction.type === TransactionType.Income) {
        stats.income += amount
      } else {
        stats.expenses += amount
      }

      const analysis = analyzeTransaction(transaction)

      if (analysis.isNutrition) {
        stats.nutrition += amount
        if (analysis.nutritionKeyword) {
          nutritionFamilies.add(analysis.nutritionKeyword)
        }
      }

      if (analysis.isImpulse) {
        stats.impulseSpending += amount
        stats.impulseTransactions += 1
      }

      const dayKey = toDayKey(transaction.date)
      if (dayKey) {
        const bucket = dayBuckets.get(dayKey) ?? { income: 0, expenses: 0, nutrition: 0, count: 0 }
        if (transaction.type === TransactionType.Income) {
          bucket.income += amount
        } else {
          bucket.expenses += amount
        }
        if (analysis.isNutrition) {
          bucket.nutrition += amount
        }
        bucket.count += 1
        dayBuckets.set(dayKey, bucket)
      }

      const txTimestamp = new Date(transaction.date).getTime()
      if (!Number.isNaN(txTimestamp) && txTimestamp >= weekThreshold) {
        stats.weekTransactions += 1
      }
    })

    stats.nutritionFamilies = nutritionFamilies.size
    stats.dailySnapshots = Array.from(dayBuckets.entries())
      .sort(([a], [b]) => a.localeCompare(b))
      .map(([date, bucket]) => ({ date, ...bucket }))

    return stats
  })

  const financeScore = computed(() => {
    const { income, expenses } = totals.value
    if (income === 0 && expenses === 0) return 50
    const savingsRatio = income === 0 ? -1 : (income - expenses) / Math.max(income, 1)
    return clamp(60 + savingsRatio * 40)
  })

  const nutritionScore = computed(() => {
    const { expenses, nutrition, nutritionFamilies } = totals.value
    if (expenses === 0) return clamp(35 + nutritionFamilies * 5)
    const nutritionShare = nutrition / expenses
    const diversityBonus = Math.min(1, nutritionFamilies / 4)
    return clamp(40 + nutritionShare * 40 + diversityBonus * 20)
  })

  const nutritionSharePercent = computed(() => {
    const { expenses, nutrition } = totals.value
    if (expenses === 0) return 0
    return clamp((nutrition / expenses) * 100)
  })

  const velocityScore = computed(() => {
    const goal = 14
    const { weekTransactions } = totals.value
    if (weekTransactions === 0) return 20
    return clamp((weekTransactions / goal) * 100)
  })

  const velocityLabel = computed(() => {
    if (velocityScore.value < 35) return 'slow & steady'
    if (velocityScore.value < 75) return 'in rhythm'
    return 'hyper active'
  })

  const greenDayCount = computed(() => {
    return totals.value.dailySnapshots.filter((day) => {
      if (day.expenses === 0) return false
      const financeGreen = day.income >= day.expenses * 0.85
      const nutritionGreen = day.nutrition >= day.expenses * 0.25
      return financeGreen && nutritionGreen
    }).length
  })

  const greenStreak = computed(() => {
    const snapshots = [...totals.value.dailySnapshots].sort((a, b) => a.date.localeCompare(b.date))
    let streak = 0
    for (let index = snapshots.length - 1; index >= 0; index -= 1) {
      const day = snapshots[index]
      if (day.expenses === 0) break
      const financeGreen = day.income >= day.expenses * 0.85
      const nutritionGreen = day.nutrition >= day.expenses * 0.25
      if (financeGreen && nutritionGreen) streak += 1
      else break
    }
    return streak
  })

  const mutationReady = computed(() => financeScore.value >= 70 && nutritionScore.value >= 70 && greenStreak.value >= 5)

  const progressBars = computed<TamagotchiProgressBar[]>(() => [
    {
      id: 'finance',
      label: 'Financial discipline',
      value: financeScore.value,
      tone: financeScore.value >= 70 ? 'positive' : financeScore.value >= 50 ? 'warning' : 'critical',
      description: `Savings rate ${(financeScore.value - 50).toFixed(0)} pts`,
    },
    {
      id: 'nutrition',
      label: 'Nutrition balance',
      value: nutritionScore.value,
      tone: nutritionScore.value >= 70 ? 'positive' : nutritionScore.value >= 50 ? 'warning' : 'critical',
      description: `${nutritionSharePercent.value}% of spend`,
    },
  ])

  const seasonalMutation = computed<SeasonalMutationState>(() => {
    const month = new Date().getMonth()
    const currentSeason =
      SEASONS.find((season) => season.months.includes(month)) ??
      SEASONS[0]

    const requirement =
      mutationReady.value && greenDayCount.value >= 7
        ? 'Ready for new skin'
        : `Need ${Math.max(0, 7 - greenDayCount.value)} green days`

    return {
      season: currentSeason.season,
      aura: currentSeason.aura,
      focus: currentSeason.focus,
      ready: mutationReady.value,
      summary: mutationReady.value
        ? `All systems synced for the ${currentSeason.season} mutation`
        : 'Keep both bars in the green to unlock the mutation',
      requirement,
    }
  })

  const tamagotchiStage = computed<TamagotchiStage>(() => {
    if (mutationReady.value) {
      return {
        name: 'Seasonal Guardian',
        tier: 'guardian',
        descriptor: 'Balanced finance & nutrition unlocked a rare form',
        moodLabel: 'Focused joy',
        velocityLabel: velocityLabel.value,
        stability: clamp((financeScore.value + nutritionScore.value) / 2),
      }
    }

    if (financeScore.value >= 80 && nutritionScore.value >= 65) {
      return {
        name: 'Balanced Scout',
        tier: 'scout',
        descriptor: 'Spending rhythm feels healthy and curious',
        moodLabel: 'Optimistic',
        velocityLabel: velocityLabel.value,
        stability: clamp((financeScore.value + nutritionScore.value) / 2),
      }
    }

    if (financeScore.value < 50) {
      return {
        name: 'Recovery Cocoon',
        tier: 'cocoon',
        descriptor: 'Creature slowed down to rebuild confidence',
        moodLabel: 'Cautious',
        velocityLabel: velocityLabel.value,
        stability: clamp((financeScore.value + nutritionScore.value) / 2),
      }
    }

    return {
      name: 'Focused Sprout',
      tier: 'sprout',
      descriptor: 'Finding footing through steady routines',
      moodLabel: 'Determined',
      velocityLabel: velocityLabel.value,
      stability: clamp((financeScore.value + nutritionScore.value) / 2),
    }
  })

  const tamagotchiMood = computed<TamagotchiMood>(() => {
    if (mutationReady.value) {
      return {
        label: 'Radiant',
        aura: '#9b7bff',
        intensity: 1,
      }
    }

    if (financeScore.value < 50) {
      return {
        label: 'Worried',
        aura: '#ff9f7a',
        intensity: 0.6,
      }
    }

    if (nutritionScore.value < 50) {
      return {
        label: 'Hungry',
        aura: '#ffc857',
        intensity: 0.6,
      }
    }

    return {
      label: 'Playful',
      aura: '#6be4c7',
      intensity: 0.85,
    }
  })

  const transactionReactions = computed<Record<string, TransactionReaction>>(() => {
    const ledger: Record<string, TransactionReaction> = {}

    normalizedTransactions.value.forEach((transaction) => {
      const analysis = analyzeTransaction(transaction)
      const magnitude = Math.abs(transaction.amount)
      const relative = totals.value.expenses === 0 ? 0.3 : Math.min(1, magnitude / Math.max(totals.value.expenses, 1) * 4)

      let tone: ReactionTone = 'focus'
      if (transaction.type === TransactionType.Income) tone = 'celebrate'
      else if (analysis.isNutrition) tone = 'nourish'
      else if (analysis.isImpulse || transaction.status === TransactionStatus.Error) tone = 'concern'
      else if (transaction.status === TransactionStatus.Pending) tone = 'calm'

      const message =
        tone === 'celebrate'
          ? `Saved +${formatShortAmount(magnitude)}`
          : tone === 'nourish'
            ? `Fuel from ${transaction.category}`
            : tone === 'concern'
              ? `Impulse: ${transaction.category}`
              : tone === 'calm'
                ? 'Waiting to clear'
                : `Focused spend on ${transaction.category}`

      ledger[transaction.uuid] = {
        tone,
        color: TONE_COLORS[tone],
        message,
        intensity: relative,
        isNutrition: analysis.isNutrition,
      }
    })

    return ledger
  })

  const reactionTrend = computed<ReactionTrendEntry[]>(() => {
    const latest = normalizedTransactions.value.slice(-5)
    return latest.map((transaction) => {
      const reaction = transactionReactions.value[transaction.uuid]
      return {
        id: transaction.uuid,
        tone: reaction?.tone ?? 'focus',
        color: reaction?.color ?? '#d1d5db',
      }
    })
  })

  const achievements = computed<AchievementBadge[]>(() => {
    const impulseRatio =
      totals.value.transactionCount === 0
        ? 0
        : totals.value.impulseTransactions / Math.max(totals.value.transactionCount, 1)

    const calmProgress = clamp((1 - impulseRatio) * 100)

    return [
      {
        id: 'dual-green-weeks',
        name: 'Three Green Weeks',
        rarity: 'epic',
        tags: ['finance', 'nutrition'],
        progress: clamp((greenStreak.value / 21) * 100),
        description: 'Keep both progress bars in the green for 21 consecutive days',
        limited: true,
        timeRemaining: '6d 12h',
      },
      {
        id: 'nutrition-collector',
        name: 'Palette Explorer',
        rarity: 'rare',
        tags: ['nutrition'],
        progress: clamp((totals.value.nutritionFamilies / 5) * 100),
        description: 'Log purchases across five nutrition categories',
      },
      {
        id: 'impulse-tamer',
        name: 'Impulse Tamer',
        rarity: 'common',
        tags: ['finance'],
        progress: calmProgress,
        description: 'Keep impulse alerts under control for 10 transactions',
      },
      {
        id: 'seasonal-morph',
        name: 'Seasonal Mutation',
        rarity: 'mythic',
        tags: ['finance', 'nutrition'],
        progress: mutationReady.value ? 100 : clamp((greenDayCount.value / 7) * 100),
        description: 'Unlock the next Tamagotchi skin by sustaining dual-green days',
        limited: true,
        timeRemaining: '4d 03h',
      },
    ]
  })

  const merchInventory = computed<MerchItem[]>(() => {
    const duplicatesFromImpulse = Math.max(0, Math.floor(totals.value.impulseTransactions / 2))
    const quizBoosters = nutritionScore.value >= 70 ? 2 : 1
    return [
      {
        id: 'calming-scarf',
        name: 'Calming Scarf',
        rarity: 'rare',
        slot: 'aura',
        effect: '-15% negative reaction intensity',
        copies: 1,
        duplicates: Math.min(3, duplicatesFromImpulse),
        fusionReady: duplicatesFromImpulse >= 2,
        perk: 'Stabilises concern spikes',
        exchangeCost: 2,
      },
      {
        id: 'budget-lens',
        name: 'Budget Lens',
        rarity: 'common',
        slot: 'reaction',
        effect: '+5% finance progress after quizzes',
        copies: 2,
        duplicates: 1,
        fusionReady: false,
        perk: 'Extends quiz reward duration',
        exchangeCost: 1,
      },
      {
        id: 'garden-token',
        name: 'Garden Token',
        rarity: 'epic',
        slot: 'quiz',
        effect: '+10% nutrition boost on drag mini-game',
        copies: quizBoosters,
        duplicates: Math.max(0, quizBoosters - 1),
        fusionReady: quizBoosters > 2,
        perk: 'Doubles produce prompts',
        exchangeCost: 3,
      },
    ]
  })

  const miniGames = computed<MiniGame[]>(() => [
    {
      id: 'needs-wants-sorter',
      title: 'Needs vs Wants Sorter',
      type: 'drag',
      description: 'Drag transactions into needs/wants/nutrition buckets',
      reward: '+6 finance XP, +4 nutrition XP',
      status: 'available',
      boostTarget: 'finance',
    },
    {
      id: 'macro-memory',
      title: 'Macro Memory Match',
      type: 'tempo',
      description: 'Match ingredients to macro groups based on your grocery history',
      reward: '+8 nutrition XP',
      status: nutritionScore.value >= 70 ? 'available' : 'cooldown',
      boostTarget: 'nutrition',
    },
    {
      id: 'reaction-mapper',
      title: 'Reaction Mapper',
      type: 'quiz',
      description: 'Predict creature mood for the last 5 transactions',
      reward: '+4 mood XP',
      status: 'available',
      boostTarget: 'mood',
    },
  ])

  const quizQueue = computed<QuizCard[]>(() => {
    const lastExpense = [...normalizedTransactions.value].reverse().find((transaction) => transaction.type === TransactionType.Expense)
    const lockingCategory = lastExpense?.category ?? 'spending mix'

    return [
      {
        id: 'budget-check',
        title: 'Cashflow Pulse',
        topic: 'Income vs expenses quiz',
        reward: '+5% finance progress',
        boostTarget: 'finance',
        lockingCategory,
      },
      {
        id: 'nutrition-quiz',
        title: 'Fuel Forecast',
        topic: 'Pick the most nutritious swap based on your groceries',
        reward: '+5% nutrition progress',
        boostTarget: 'nutrition',
        lockingCategory: 'groceries',
      },
    ]
  })

  const nutritionPrompts = computed<NutritionPrompt[]>(() => {
    const targetShare = 35
    const deficit = Math.max(0, targetShare - nutritionSharePercent.value)

    const prompt: NutritionPrompt = {
      goal: `Weekly meal plan sync (${nutritionSharePercent.value}% / ${targetShare}%)`,
      status: deficit === 0 ? 'On track' : 'Needs boost',
      action: deficit === 0 ? 'Lock gains with a pantry audit' : 'Add one produce-heavy receipt to reach the target',
      highlight: totals.value.nutritionFamilies >= 4 ? 'Diversified plate unlocked' : 'Try a new nutrition category',
      onTrack: deficit === 0,
    }

    const hydrationPrompt: NutritionPrompt = {
      goal: 'Hydration streak',
      status: totals.value.nutritionFamilies >= 3 ? 'Stable' : 'Low',
      action: 'Log a beverage purchase to keep hydration streak alive',
      highlight: 'Hydration buffs mood reactions',
      onTrack: totals.value.nutritionFamilies >= 3,
    }

    return [prompt, hydrationPrompt]
  })

  const headerBadges = computed<HeaderBadges>(() => ({
    achievements: achievements.value.filter((achievement) => achievement.progress >= 100).length,
    merch: merchInventory.value.filter((item) => item.fusionReady).length,
  }))

  return {
    progressBars,
    seasonalMutation,
    tamagotchiStage,
    tamagotchiMood,
    transactionReactions,
    reactionTrend,
    achievements,
    merchInventory,
    miniGames,
    quizQueue,
    nutritionPrompts,
    headerBadges,
  }
}

