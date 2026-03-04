import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { StatisticDto } from '../models/statistic.model';

@Injectable({
  providedIn: 'root',
})
export class StatisticService {
  http = inject(HttpClient);
  
    apiUrl = environment.baseUrl + '/Statistic';
  
    fetchStatistics(): Observable<StatisticDto> {
      return this.http.get<StatisticDto>(this.apiUrl)
    }
}
