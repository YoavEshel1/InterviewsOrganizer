import { DatePipe } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { Position, positionStatusLabel } from '../../models/position';

@Component({
  selector: 'app-position-item',
  imports: [DatePipe],
  templateUrl: './position-item.html',
  styleUrl: './position-item.scss',
  host: {
    class: 'position-item',
    '[class.selected]': 'selected()',
    '(click)': 'selectPosition()',
  },
})
export class PositionItem {
  readonly position = input.required<Position>();
  readonly selected = input(false);
  readonly positionSelected = output<string>();

  protected readonly statusLabel = positionStatusLabel;

  protected selectPosition(): void {
    this.positionSelected.emit(this.position().id);
  }
}
