import { CommonModule } from '@angular/common';
import { Component, computed, inject, input } from '@angular/core';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { XpData } from '../../../models/statistic.model';
import { AuthService } from '../../../services/auth-service';

@Component({
  selector: 'app-leaderboard-panel',
  imports: [CardModule, TableModule, CommonModule],
  templateUrl: './leaderboard-panel.html',
  styles: ``,
})
export class LeaderboardPanel {
  private auth = inject(AuthService);

  data = input.required<XpData>();

  displayedEntries = computed(() => {
    const data = this.data();
    const currentUserId = this.auth.userId();
    const currentUsername = this.auth.username();

    const entries = data.topUsers.map((u, index) => ({
      ...u,
      rank: index + 1,
      isCurrentUser: currentUserId === u.userId,
    }));

    const isInTop = entries.some((e) => e.isCurrentUser);

    if (!isInTop && currentUserId) {
      entries.push({
        userId: currentUserId,
        username: currentUsername ?? currentUserId,
        totalXp: data.currentUserXp,
        rank: data.currentUserRank,
        isCurrentUser: true,
      });
    }

    return entries;
  });
}
