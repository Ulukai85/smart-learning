import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { CreateDeckDto, DeckDto } from '../models/deck.model';

@Injectable({
  providedIn: 'root',
})
export class DeckService {
  http = inject(HttpClient);

  apiUrl = environment.baseUrl + '/Deck';

  public getAllDecks(): Observable<DeckDto[]> {
    return this.http.get<DeckDto[]>(this.apiUrl);
  }

  public createDeck(dto: CreateDeckDto): Observable<DeckDto> {
    return this.http.post<DeckDto>(this.apiUrl, dto);
  }
}
