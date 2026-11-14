import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './auth-layout.html',
  styleUrl: './auth-layout.css',
})
export class AuthLayoutComponent {
  eyebrow = input.required<string>();
  headline = input.required<string>();
  description = input.required<string>();
}
