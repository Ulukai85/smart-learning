import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { UpsertDeckDto, DeckDto, DeckSummaryDto } from '../models/deck.model';

@Injectable({
  providedIn: 'root',
})
export class DeckService {
  http = inject(HttpClient);

  apiUrl = environment.baseUrl + '/Decks';

  public getDecksForUser(): Observable<DeckDto[]> {
    return this.http.get<DeckDto[]>(this.apiUrl);
  }

  public getPublicDecks(): Observable<DeckDto[]> {
    return this.http.get<DeckDto[]>(this.apiUrl + '/public');
  }

  public createDeck(dto: UpsertDeckDto): Observable<DeckDto> {
    return this.http.post<DeckDto>(this.apiUrl, dto);
  }

  public updateDeck(id: string, dto: UpsertDeckDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  public getDeckSummary(): Observable<DeckSummaryDto[]> {
    return this.http.get<DeckSummaryDto[]>(`${this.apiUrl}/summary`);
  }

  public toggleIsPublished(dto: DeckSummaryDto): Observable<void> {
    if (dto.isPublished) {
      return this.http.patch<void>(`${this.apiUrl}/${dto.id}/publish`, null);
    }
    return this.http.patch<void>(`${this.apiUrl}/${dto.id}/unpublish`, null);
  }
}
