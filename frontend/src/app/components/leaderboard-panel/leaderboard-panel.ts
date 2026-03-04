import { Component, computed, input } from '@angular/core';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { XpData } from '../../models/statistic.model';

@Component({
  selector: 'app-leaderboard-panel',
  imports: [CardModule, TableModule],
  templateUrl: './leaderboard-panel.html',
  styles: ``,
})
export class LeaderboardPanel {
  data = input.required<XpData>();

  isCurrentUserInList = computed(() => this.data().currentUserRank <= 5)

  displayedEntries = computed(() => {
    const data = this.data();

    const entries = data.topUsers.map((u, index) => ({
      ...u,
      rank: index + 1,
      isCurrentUser: this.data().currentUserRank === index + 1
    }))

    if (!this.isCurrentUserInList()) {
      entries.push({
        username: data.
      })
    }
  })
}
