import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { StatisticService } from '../../services/statistic-service';
import { StatisticDto } from '../../models/statistic.model';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.html',
  styles: ``,
})
export class Dashboard implements OnInit {
  private statisticService = inject(StatisticService);
  statistics = signal<StatisticDto | null>(null)
  streak = computed(() => this.statistics()?.streakData)
  xp = computed(() => this.statistics()?.xpData)

  ngOnInit(): void {
    this.loadStatistics()
  }

  loadStatistics() {
    this.statisticService.fetchStatistics().subscribe({
      next: data => {
        this.statistics.set(data);
        console.log('statistic:', this.statistics())
        console.log('streak:', this.streak())
        console.log('xp:', this.xp())
      },
      error: err => console.log('Err: ', err)
    })
  }
}
