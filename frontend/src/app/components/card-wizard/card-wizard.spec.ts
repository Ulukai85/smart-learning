import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CardWizard } from './card-wizard';

describe('CardWizard', () => {
  let component: CardWizard;
  let fixture: ComponentFixture<CardWizard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CardWizard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CardWizard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
