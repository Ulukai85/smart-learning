import { Component, input, OnInit } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ChartModule } from 'primeng/chart';
import { DailyReviewDto } from '../../../models/statistic.model';

@Component({
  selector: 'app-card-chart-panel',
  imports: [CardModule, ChartModule],
  templateUrl: './card-chart-panel.html',
  styles: ``,
})
export class CardChartPanel implements OnInit {
  data = input.required<DailyReviewDto[]>();

  chartData: any;
  chartOptions: any;

  ngOnInit(): void {
    this.chartData = {
      labels: this.data().map((d) => d.date),
      datasets: [
        {
          label: 'Cards Reviewed',
          data: this.data().map((d) => d.cardsReviewed),
          fill: false,
          tension: 0.4,
        },
      ],
    };

    this.chartOptions = {
      responsive: true,
      maintainAspectRatio: false,
    };
  }
}
