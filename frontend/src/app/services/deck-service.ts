import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { UpsertDeckDto, DeckDto } from '../models/deck.model';

@Injectable({
  providedIn: 'root',
})
export class DeckService {
  http = inject(HttpClient);

  apiUrl = environment.baseUrl + '/Decks';

  public getAllDecks(): Observable<DeckDto[]> {
    return this.http.get<DeckDto[]>(this.apiUrl);
  }

  public createDeck(dto: UpsertDeckDto): Observable<DeckDto> {
    return this.http.post<DeckDto>(this.apiUrl, dto);
  }

  public updateDeck(id: string, dto: UpsertDeckDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }
}
