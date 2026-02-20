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

  fetchCardBatch(deckId: string, dueLimit = 20, newLimit = 10): Observable<CardToReviewDto[]> {
    const params = new HttpParams().set('newLimit', newLimit).set('dueLimit', dueLimit);
    return this.http.get<CardToReviewDto[]>(`${this.apiUrl}/deck/${deckId}`, { params });
  }

  saveCardReview() {}
}
