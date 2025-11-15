export type UiLanguage = 'en' | 'ru'

export const availableLanguages: Array<{ id: UiLanguage; label: string; nativeLabel: string }> = [
  { id: 'en', label: 'English', nativeLabel: 'English' },
  { id: 'ru', label: 'Russian', nativeLabel: 'Русский' },
]

export const messages: Record<UiLanguage, Record<string, string>> = {
  en: {
    'index.openCreature': 'Open creature',
    'index.nutritionStats': 'Nutrition stats',

    'tamagotchi.miniGames.title': 'Mini-games',
    'tamagotchi.miniGames.subtitle': 'Boost specific progress bars with short interactions',
    'tamagotchi.quiz.title': 'Upcoming quizzes',
    'tamagotchi.quiz.subtitle': 'Answer to unlock temporary buffs right after sensitive transactions',
    'tamagotchi.nutrition.title': 'Nutrition reinforcement',
    'tamagotchi.nutrition.subtitle': 'Creature mood mirrors how you shop for food',
    'tamagotchi.challenges.title': 'Limited challenges',
    'tamagotchi.challenges.subtitle': 'Combo achievements with dual tags',
    'tamagotchi.recentStreak': 'Recent emotional streak',

    'statistics.nutrition.title': 'Meal plan sync',
    'statistics.nutrition.subtitle': 'Align grocery behaviour with creature well-being',
    'statistics.overview.title': 'Progress overview',
    'statistics.overview.subtitle': 'Finance + nutrition signal health',

    'inventory.copies': 'Copies',
    'inventory.duplicates': 'Duplicates',
    'inventory.fusionCost': 'Fusion cost',
    'inventory.readyToFuse': 'Ready to fuse',
    'inventory.needMoreShards': 'Need more shards',
    'inventory.collectMore': 'Collect more items',
    'inventory.slotLabel': 'Slot',
    'inventory.rarityLabel': 'Rarity',
    'inventory.fuseAction': 'Fuse duplicates',

    'achievements.detail.viewRequirements': 'View requirements',

    'settings.pageTitle': 'Settings & Preferences',
    'settings.languageTitle': 'Interface language',
    'settings.languageDescription': 'Choose how Labubu speaks to you',
    'settings.accountTitle': 'Account',
    'settings.accountDescription': 'Manage login and personal data',
    'settings.logoutTitle': 'Logout',
    'settings.logoutDescription': 'Disconnect this device from your Labubu profile',
    'settings.logoutButton': 'Log out',
    'settings.languageSelected': 'Selected',
  },
  ru: {
    'index.openCreature': 'Открыть питомца',
    'index.nutritionStats': 'Статистика питания',

    'tamagotchi.miniGames.title': 'Мини-игры',
    'tamagotchi.miniGames.subtitle': 'Укрепляйте прогресс короткими сессиями',
    'tamagotchi.quiz.title': 'Ближайшие квизы',
    'tamagotchi.quiz.subtitle': 'Отвечайте, чтобы получить временные бонусы',
    'tamagotchi.nutrition.title': 'Питательные привычки',
    'tamagotchi.nutrition.subtitle': 'Настроение питомца отражает ваши покупки',
    'tamagotchi.challenges.title': 'Ограниченные испытания',
    'tamagotchi.challenges.subtitle': 'Комбо-достижения для финансов и питания',
    'tamagotchi.recentStreak': 'Последняя эмоциональная полоса',

    'statistics.nutrition.title': 'Синхронизация рациона',
    'statistics.nutrition.subtitle': 'Согласуйте покупки с самочувствием питомца',
    'statistics.overview.title': 'Общий прогресс',
    'statistics.overview.subtitle': 'Финансы и питание в одном взгляде',

    'inventory.copies': 'Копии',
    'inventory.duplicates': 'Дубликаты',
    'inventory.fusionCost': 'Цена слияния',
    'inventory.readyToFuse': 'Готово к слиянию',
    'inventory.needMoreShards': 'Нужно больше шардов',
    'inventory.collectMore': 'Соберите ещё предметы',
    'inventory.slotLabel': 'Слот',
    'inventory.rarityLabel': 'Редкость',
    'inventory.fuseAction': 'Слить дубликаты',

    'achievements.detail.viewRequirements': 'Посмотреть условия',

    'settings.pageTitle': 'Настройки и предпочтения',
    'settings.languageTitle': 'Язык интерфейса',
    'settings.languageDescription': 'Выберите, как Лабубу с вами говорит',
    'settings.accountTitle': 'Аккаунт',
    'settings.accountDescription': 'Управление входом и личными данными',
    'settings.logoutTitle': 'Выход',
    'settings.logoutDescription': 'Отключить это устройство от профиля Labubu',
    'settings.logoutButton': 'Выйти',
    'settings.languageSelected': 'Выбран',
  },
}


