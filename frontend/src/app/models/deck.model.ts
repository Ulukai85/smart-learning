import { CardToReviewDto } from './card.model';

export interface DeckDto {
  id: string;
  ownerUserId: string;
  name: string;
  description: string;
  totalCards: number;
}

export interface UpsertDeckDto {
  name: string;
  description: string;
}

export interface DeckSummaryDto {
  id: string;
  name: string;
  totalCards: number;
  isPublished: boolean;
  isForked: boolean;
  newCards: number;
  dueCards: number;
}

export interface DeckToReviewDto {
  id: string;
  name: string;
  newCards: number;
  dueCards: number;
  cards: CardToReviewDto[];
}
