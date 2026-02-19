import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { CardToReviewDto } from '../models/card.model';

@Injectable({
  providedIn: 'root',
})
export class ReviewService {
  http = inject(HttpClient);

  apiUrl = environment.baseUrl + '/Review';
  
  getCardBatch(deckId: string,newCards: number = 10, dueCards: number = 20): Observable<CardToReviewDto[]> {
    const params = new HttpParams()
    .set('new', newCards)
    .set('due', dueCards);
    return this.http.get<CardToReviewDto[]>(`${this.apiUrl}/deck/${deckId}`, {params})
  }
}
