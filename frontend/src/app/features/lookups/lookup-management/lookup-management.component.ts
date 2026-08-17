import { Component } from '@angular/core';

import { LookupCardComponent } from '../lookup-card/lookup-card.component';

@Component({
  selector: 'app-lookup-management',
  standalone: true,
  imports: [LookupCardComponent],
  template: `
    <h1 class="lookups__title">Settings</h1>
    <p class="lookups__subtitle">Manage the job titles and departments contacts can be assigned to.</p>

    <div class="lookups__grid">
      <app-lookup-card title="Job Titles" kind="jobTitle" />
      <app-lookup-card title="Departments" kind="department" />
    </div>
  `,
  styles: [
    `
      .lookups__title {
        margin: 0 0 4px;
        font-size: 1.5rem;
        font-weight: 500;
      }

      .lookups__subtitle {
        margin: 0 0 24px;
        opacity: 0.7;
      }

      .lookups__grid {
        display: grid;
        grid-template-columns: repeat(2, minmax(0, 1fr));
        gap: 24px;
        align-items: start;
      }

      /* Side by side on desktop, stacked on a phone. */
      @media (max-width: 899px) {
        .lookups__grid {
          grid-template-columns: 1fr;
        }
      }
    `
  ]
})
export class LookupManagementComponent {}
