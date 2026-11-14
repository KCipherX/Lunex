import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService, Toast } from '../../../core/services/toast.service';
import { trigger, transition, style, animate } from '@angular/animations';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="fixed top-0 right-0 z-50 p-4 w-full max-w-sm pointer-events-none">
      @for (toast of toastService.allToasts(); track toast.id) {
        <div 
          class="toast-item pointer-events-auto"
          [@toastAnimation]
        >
          <div [class]="toastClasses(toast)">
            <div class="flex items-start gap-3">
              <div class="flex-shrink-0 mt-0.5">
                <ng-container [ngSwitch]="toast.type">
                  <svg *ngSwitchCase="'success'" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-success" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  <svg *ngSwitchCase="'error'" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-error" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  <svg *ngSwitchCase="'warning'" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-warning" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                  </svg>
                  <svg *ngSwitchCase="'info'" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-info" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </ng-container>
              </div>
              
              <div class="flex-1 min-w-0">
                @if (toast.title) {
                  <p class="text-sm font-semibold text-base-content">{{ toast.title }}</p>
                }
                @if (toast.message) {
                  <p class="text-sm text-base-content/70 mt-0.5">{{ toast.message }}</p>
                }
              </div>
              
              <button
                type="button"
                class="flex-shrink-0 text-base-content/50 hover:text-base-content transition-colors"
                (click)="toastService.dismiss(toast.id)"
              >
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  animations: [
    trigger('toastAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateX(100%)', height: 0 }),
        animate('300ms cubic-bezier(0.4, 0, 0.2, 1)', style({ opacity: 1, transform: 'translateX(0)', height: '*' })),
      ]),
      transition(':leave', [
        animate('200ms ease-in', style({ opacity: 0, transform: 'translateX(100%)', height: 0 })),
      ]),
    ]),
  ],
})
export class ToastContainerComponent {
  toastService = inject(ToastService);

  toastClasses(toast: Toast): string {
    const base = 'bg-base-100 rounded-xl shadow-lg border p-4 animate-slide-in';
    
    const typeClasses = {
      success: 'border-l-4 border-success',
      error: 'border-l-4 border-error',
      warning: 'border-l-4 border-warning',
      info: 'border-l-4 border-info',
    };

    return `${base} ${typeClasses[toast.type]}`;
  }
}