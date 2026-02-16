import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CardDto, UpsertCardDto } from '../models/card.model';

@Injectable({
  providedIn: 'root',
})
export class CardService {
  http = inject(HttpClient);

  apiUrl = environment.baseUrl + '/Cards';

  public getAllCards(): Observable<CardDto[]> {
    return this.http.get<CardDto[]>(this.apiUrl);
  }

  public createCard(dto: UpsertCardDto): Observable<void> {
    return this.http.post<void>(this.apiUrl, dto);
  }

  public updateCard(id: string, dto: UpsertCardDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  public deleteCard(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
