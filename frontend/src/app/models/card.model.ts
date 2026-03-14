export interface CardDto {
  id: string;
  deckId: string;
  deckName: string;
  front: string;
  back: string;
}

export interface UpsertCardDto {
  deckId: string;
  front: string;
  back: string;
}

export interface CardToReviewDto {
  id: string;
  front: string;
  back: string;
  nextReviewAt?: string;
  isNew: boolean;
  strategyType: string;
  strategyData: string;
}

export interface AiCreateCardsDto {
  count: number;
  topic: string;
  description: string;
  deckId?: string;
}

export interface AiCard {
  front: string;
  back: string;
}

export interface AiCardResponse {
  cards: AiCard[];
}
