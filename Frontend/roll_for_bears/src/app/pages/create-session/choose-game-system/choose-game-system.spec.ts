import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChooseGameSystem } from './choose-game-system';

describe('ChooseGameSystem', () => {
  let component: ChooseGameSystem;
  let fixture: ComponentFixture<ChooseGameSystem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChooseGameSystem],
    }).compileComponents();

    fixture = TestBed.createComponent(ChooseGameSystem);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
