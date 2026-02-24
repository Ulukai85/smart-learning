export interface CreateReviewTransactionDto {
  cardId: string;
  grade: number;
  strategyType?: string;
  strategyData: string;
  isNew: boolean;
}

export interface XpTransactionDto {
  id: string;
  amount: number;
  reason: string;
}

export interface ReviewResultDto {
  reviewedCardId: string,
  reinsertCard: boolean,
  xpAmount: number,
  reason: string,
  nextReviewAt: string
}