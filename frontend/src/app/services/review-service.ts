import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { DeckToReviewDto } from '../models/deck.model';

@Injectable({
  providedIn: 'root',
})
export class ReviewService {
  http = inject(HttpClient);

  apiUrl = environment.baseUrl + '/Review';

  fetchDeckToReview(deckId: string, dueLimit = 20, newLimit = 10): Observable<DeckToReviewDto> {
    const params = new HttpParams().set('newLimit', newLimit).set('dueLimit', dueLimit);
    return this.http.get<DeckToReviewDto>(`${this.apiUrl}/deck/${deckId}`, { params });
  }

  saveCardReview() {}
}
