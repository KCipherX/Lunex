import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ButtonComponent, CardComponent, AvatarComponent, IconComponent, IconName } from '../../shared/components';

interface Feature {
  icon: IconName;
  title: string;
  description: string;
}

interface Stat {
  value: string;
  label: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, ButtonComponent, CardComponent, AvatarComponent, IconComponent],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class HomeComponent {
  scrollToFeatures() {
    document.getElementById('features')?.scrollIntoView({ behavior: 'smooth' });
  }

  features: Feature[] = [
    {
      icon: 'users',
      title: 'Find Matches',
      description: 'Discover people who share your interests and values with our smart matching algorithm.',
    },
    {
      icon: 'chat',
      title: 'Start Conversations',
      description: 'Break the ice with personalized messages and build meaningful connections.',
    },
    {
      icon: 'shield-check',
      title: 'Secure & Private',
      description: 'Your privacy is our priority. All data is encrypted and you control what you share.',
    },
  ];

  stats: Stat[] = [
    { value: '10K+', label: 'Active Members' },
    { value: '50K+', label: 'Connections Made' },
    { value: '95%', label: 'Match Success Rate' },
    { value: '24/7', label: 'Support Available' },
  ];

  trackByStatLabel(index: number, stat: Stat): string {
    return stat.label;
  }

  trackByFeatureTitle(index: number, feature: Feature): string {
    return feature.title;
  }
}