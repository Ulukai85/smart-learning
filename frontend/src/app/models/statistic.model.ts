export interface StatisticDto {
    streakData: StreakData;
    xpData: XpData
}

export interface StreakData {
  longestStreak: number;
  currentStreak: number;
  reviewDates: string[];
}

export interface XpData {
  currentUserXp: number;
  currentUserRank: number;
  topUsers: LeaderboardEntryData[]
}

export interface LeaderboardEntryData {
  userId: string;
  username: string;
  totalXp: number
}