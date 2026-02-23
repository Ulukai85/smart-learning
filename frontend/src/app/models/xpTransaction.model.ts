export interface CreateReviewTransactionDto {
    cardId: string,
    grade: number,
    strategyType: string
}

export interface XpTransactionDto {
    id: string,
    amount: number,
    reason: string
}