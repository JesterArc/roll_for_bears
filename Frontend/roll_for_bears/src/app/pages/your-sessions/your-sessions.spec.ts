import { ComponentFixture, TestBed } from '@angular/core/testing';
import { YourSessions } from './your-sessions';

describe('YourSessions', () => {
  let component: YourSessions;
  let fixture: ComponentFixture<YourSessions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [YourSessions],
    }).compileComponents();

    fixture = TestBed.createComponent(YourSessions);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
