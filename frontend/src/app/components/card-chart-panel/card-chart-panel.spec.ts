import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CardChartPanel } from './card-chart-panel';

describe('CardChartPanel', () => {
  let component: CardChartPanel;
  let fixture: ComponentFixture<CardChartPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CardChartPanel]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CardChartPanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
