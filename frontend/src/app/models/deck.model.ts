export interface DeckDto {
  id: string;
  ownerUserId: string;
  name: string;
  description: string;
}

export interface CreateDeckDto {
  name: string;
  description: string;
}
