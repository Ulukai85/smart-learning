import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StreakPanel } from './streak-panel';

describe('StreakPanel', () => {
  let component: StreakPanel;
  let fixture: ComponentFixture<StreakPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StreakPanel]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StreakPanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
