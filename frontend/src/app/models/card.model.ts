export interface CardDto {
  id: string;
  deckId: string;
  front: string;
  back: string;
}

export interface CreateCardDto {
  deckId: string;
  front: string;
  back: string;
}
