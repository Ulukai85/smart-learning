export interface CreateReviewTransactionDto {
  cardId: string;
  grade: number;
  strategyType?: string;
  strategyData: string;
}

export interface XpTransactionDto {
  id: string;
  amount: number;
  reason: string;
}

export interface ReviewResultDto {
  reviewedCardId: string;
  reinsertCard: boolean;
  wasNew: boolean;
  xpAmount: number;
  xpReason: string;
  nextReviewAt: string;
  updatedDueCount: number;
  updatedNewCount: number;
}
