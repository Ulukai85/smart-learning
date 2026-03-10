export interface CreateReviewTransactionDto {
  cardId: string;
  grade: number;
  strategyType?: string;
  strategyData: string;
}

export interface XpTransactionDto {
  amount: number;
  reason: string;
}

export interface ReviewResultDto {
  reviewedCardId: string;
  reinsertCard: boolean;
  wasNew: boolean;
  xpTransactions: XpTransactionDto[];
  nextReviewAt: string;
  updatedDueCount: number;
  updatedNewCount: number;
}
