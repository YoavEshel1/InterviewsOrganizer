import { Component, computed, effect, input, output, signal, untracked, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { Position } from '../../models/position';
import { PositionForm } from '../position-form/position-form';
import { PositionList } from '../position-list/position-list';

@Component({
  selector: 'app-position-area',
  imports: [MatButtonModule, PositionForm, PositionList],
  templateUrl: './position-area.html',
  styleUrl: './position-area.scss',
})
export class PositionArea {
  readonly companyId = input.required<string>();
  readonly saved = output<Position>();

  protected readonly canAddPosition = computed(() => !!this.companyId());
  protected readonly showPositionForm = signal(false);
  //we use viewChild to get a reference to the PositionList component so we can call its reload method
  //keep the tokenValue only within the PositionList component to trigger its reload method
  private readonly positionList = viewChild(PositionList);

  constructor() {
    effect(() => {
      this.companyId();
      untracked(() => this.showPositionForm.set(false));
    });
  }

  protected addPosition(): void {
    if (this.canAddPosition()) {
      this.showPositionForm.set(true);
    }
  }

  protected onPositionSaved(position: Position): void {
    this.positionList()?.reload();
    this.showPositionForm.set(false);
    this.saved.emit(position);
  }
}
