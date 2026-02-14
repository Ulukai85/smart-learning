import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CardDesigner } from './card-designer';

describe('CardDesigner', () => {
  let component: CardDesigner;
  let fixture: ComponentFixture<CardDesigner>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CardDesigner]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CardDesigner);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
