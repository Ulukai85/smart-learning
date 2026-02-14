import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CardDto, CreateCardDto } from '../models/card.model';

@Injectable({
  providedIn: 'root',
})
export class CardService {
  http = inject(HttpClient);

  apiUrl = environment.baseUrl + '/Card';

  public getAllCards(): Observable<CardDto[]> {
    return this.http.get<CardDto[]>(this.apiUrl);
  }

  public createCard(dto: CreateCardDto): Observable<CardDto> {
    return this.http.post<CardDto>(this.apiUrl, dto);
  }
}
