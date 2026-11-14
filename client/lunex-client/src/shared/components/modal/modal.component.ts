import { Component, input, output, ViewChild, ElementRef, HostListener, computed } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (isOpen()) {
      <div 
        class="fixed inset-0 z-50 flex items-center justify-center p-4"
        (click)="closeOnOverlayClick($event)"
      >
        <div 
          class="fixed inset-0 bg-black/50 backdrop-blur-sm"
          aria-hidden="true"
        ></div>
        
        <div 
          #modalContent
          class="relative w-full max-h-[90vh] overflow-hidden bg-base-100 rounded-2xl shadow-2xl"
          [class]="sizeClasses()"
          role="dialog"
          aria-modal="true"
          [attr.aria-labelledby]="title() ? 'modal-title' : null"
          [attr.aria-describedby]="description() ? 'modal-description' : null"
        >
          @if (title() || closable()) {
            <div class="flex items-center justify-between p-4 border-b border-base-200">
              @if (title()) {
                <h2 id="modal-title" class="text-lg font-semibold text-base-content">{{ title() }}</h2>
              }
              @if (closable()) {
                <button
                  type="button"
                  class="btn btn-ghost btn-sm text-base-content/60 hover:text-base-content hover:bg-base-200"
                  (click)="close.emit()"
                  aria-label="Close modal"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              }
            </div>
          }
          
          @if (description()) {
            <p id="modal-description" class="px-4 py-2 text-sm text-base-content/70 border-b border-base-200">{{ description() }}</p>
          }
          
          <div class="p-4 overflow-y-auto max-h-[calc(90vh-120px)]">
            <ng-content></ng-content>
          </div>
          
          @if (showFooter()) {
            <div class="flex items-center justify-end gap-3 p-4 border-t border-base-200 bg-base-200/50">
              <ng-content select="[modal-footer]"></ng-content>
            </div>
          }
        </div>
      </div>
    }
  `,
})
export class ModalComponent {
  isOpen = input.required<boolean>();
  title = input<string>('');
  description = input<string>('');
  size = input<'sm' | 'md' | 'lg' | 'xl' | 'full'>('md');
  closable = input(true);
  closeOnOverlay = input(true);
  showFooter = input(false);

  close = output<void>();

  @ViewChild('modalContent') modalContent!: ElementRef<HTMLDivElement>;

  sizeClasses = computed(() => {
    const sizes = {
      sm: 'max-w-sm',
      md: 'max-w-md',
      lg: 'max-w-lg',
      xl: 'max-w-xl',
      full: 'max-w-4xl',
    };
    return sizes[this.size()];
  });

  closeOnOverlayClick(event: MouseEvent) {
    if (this.closeOnOverlay() && event.target === event.currentTarget) {
      this.close.emit();
    }
  }

  @HostListener('document:keydown.escape')
  onEscape() {
    if (this.isOpen() && this.closable()) {
      this.close.emit();
    }
  }
}