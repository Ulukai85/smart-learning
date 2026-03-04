import { Component, input } from '@angular/core';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { StreakData } from '../../models/statistic.model';

@Component({
  selector: 'app-streak-panel',
  imports: [CardModule, TagModule],
  templateUrl: './streak-panel.html',
  styles: ``,
})
export class StreakPanel {
  data = input.required<StreakData>();
}
