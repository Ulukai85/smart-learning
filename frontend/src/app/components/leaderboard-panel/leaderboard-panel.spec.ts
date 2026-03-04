import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LeaderboardPanel } from './leaderboard-panel';

describe('LeaderboardPanel', () => {
  let component: LeaderboardPanel;
  let fixture: ComponentFixture<LeaderboardPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LeaderboardPanel]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LeaderboardPanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
