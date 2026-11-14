import { Component, input } from '@angular/core';

@Component({
  selector: 'app-page-header',
  standalone: true,
  template: `
    <header class="page-header">
      <div class="page-header__copy">
        @if (eyebrow()) {
          <p class="page-header__eyebrow">{{ eyebrow() }}</p>
        }
        <h1>{{ title() }}</h1>
        @if (description()) {
          <p class="page-header__description">{{ description() }}</p>
        }
      </div>
      <div class="page-header__actions">
        <ng-content select="[page-actions]"></ng-content>
      </div>
    </header>
  `,
})
export class PageHeaderComponent {
  title = input.required<string>();
  description = input('');
  eyebrow = input('');
}
