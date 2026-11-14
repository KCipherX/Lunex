import { Component, input, contentChild, TemplateRef, computed } from '@angular/core';
import { CommonModule } from '@angular/common';

type CardPadding = 'none' | 'sm' | 'md' | 'lg' | 'xl';
type CardVariant = 'default' | 'bordered' | 'shadow' | 'glass' | 'brand';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [class]="cardClasses()">
      @if (header()) {
        <div class="card-header">
          <ng-content select="[card-header]"></ng-content>
        </div>
      }
      
      <div class="card-body" [class]="bodyClasses()">
        <ng-content></ng-content>
      </div>
      
      @if (footer()) {
        <div class="card-footer">
          <ng-content select="[card-footer]"></ng-content>
        </div>
      }
    </div>
  `,
})
export class CardComponent {
  variant = input<CardVariant>('shadow');
  padding = input<CardPadding>('md');
  hover = input(false);
  clickable = input(false);

  header = contentChild(TemplateRef, { descendants: true });
  footer = contentChild(TemplateRef, { descendants: true });

  cardClasses = computed(() => {
    const base = 'card w-full bg-base-100 transition-all duration-200';
    
    const variants: Record<CardVariant, string> = {
      default: 'border border-base-200 shadow-sm',
      bordered: 'border border-base-300/70 shadow-sm',
      shadow: 'border border-base-200 shadow-lg shadow-base-content/5',
      glass: 'border border-white/40 bg-base-100/80 shadow-xl backdrop-blur-md',
      brand: 'border-0 bg-gradient-to-br from-primary to-secondary text-primary-content shadow-xl',
    };

    const hoverState = this.hover() || this.clickable()
      ? 'hover:-translate-y-0.5 hover:shadow-xl cursor-pointer'
      : '';

    return `${base} ${variants[this.variant()]} ${hoverState}`;
  });

  bodyClasses = computed(() => {
    const paddings: Record<CardPadding, string> = {
      none: 'p-0',
      sm: 'p-4',
      md: 'p-5',
      lg: 'p-6',
      xl: 'p-8',
    };

    return paddings[this.padding()];
  });
}