import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeckExplorer } from './deck-explorer';

describe('DeckExplorer', () => {
  let component: DeckExplorer;
  let fixture: ComponentFixture<DeckExplorer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeckExplorer]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeckExplorer);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
