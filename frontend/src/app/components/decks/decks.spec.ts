import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Decks } from './decks';
import { Mocked } from 'vitest';
import { DeckService } from '../../services/deck-service';
import { of } from 'rxjs';

describe('Decks', () => {
  let component: Decks;
  let fixture: ComponentFixture<Decks>;
  let mockDeckService: Mocked<DeckService>;

  function createDeckServiceMock(): Mocked<DeckService> {
    return {
      getDeckSummary: vi.fn().mockReturnValue(of([])),
    } as unknown as Mocked<DeckService>;
}

  beforeEach(async () => {
    mockDeckService = createDeckServiceMock();

    await TestBed.configureTestingModule({
      imports: [Decks],
      providers: [
        { provide: DeckService, useValue: mockDeckService}
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Decks);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
