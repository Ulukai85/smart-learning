export interface DeckDto {
  id: string;
  ownerUserId: string;
  name: string;
  description: string;
}

export interface UpsertDeckDto {
  name: string;
  description: string;
}
