import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AiCardResponse, AiCreateCardsDto } from '../models/card.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AiService {
  private http = inject(HttpClient);

  private apiUrl = environment.baseUrl + '/Ai';

  public createCards(dto: AiCreateCardsDto): Observable<AiCardResponse> {
      return this.http.post<AiCardResponse>(`${this.apiUrl}/create`, dto);
    }
}
