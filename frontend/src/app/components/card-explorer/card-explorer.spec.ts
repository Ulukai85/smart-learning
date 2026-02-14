import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CardExplorer } from './card-explorer';

describe('CardExplorer', () => {
  let component: CardExplorer;
  let fixture: ComponentFixture<CardExplorer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CardExplorer]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CardExplorer);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
