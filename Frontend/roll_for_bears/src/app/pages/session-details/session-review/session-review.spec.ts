import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SessionReview } from './session-review';

describe('SessionReview', () => {
  let component: SessionReview;
  let fixture: ComponentFixture<SessionReview>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SessionReview],
    }).compileComponents();

    fixture = TestBed.createComponent(SessionReview);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
