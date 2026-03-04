import { Component, inject, OnInit, signal } from '@angular/core';
import { StatisticDto } from '../../models/statistic.model';
import { StatisticService } from '../../services/statistic-service';
import { LeaderboardPanel } from '../leaderboard-panel/leaderboard-panel';
import { StreakPanel } from '../streak-panel/streak-panel';
import { CardChartPanel } from '../card-chart-panel/card-chart-panel';
import { ImageModule } from 'primeng/image';

@Component({
  selector: 'app-dashboard',
  imports: [LeaderboardPanel, StreakPanel, CardChartPanel, ImageModule],
  templateUrl: './dashboard.html',
  styles: ``,
})
export class Dashboard implements OnInit {
  private statisticService = inject(StatisticService);
  statistics = signal<StatisticDto | null>(null);

  ngOnInit(): void {
    this.loadStatistics();
  }

  loadStatistics() {
    this.statisticService.fetchStatistics().subscribe({
      next: (data) => {
        this.statistics.set(data);
        console.log('statistic:', this.statistics());
      },
      error: (err) => console.log('Err: ', err),
    });
  }
}
