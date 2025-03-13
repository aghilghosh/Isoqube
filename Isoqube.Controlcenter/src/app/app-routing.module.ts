import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DefaultComponent } from './components/default/default.component';
import { OrchestrationComponent } from './components/orchestration/orchestration.component';
import { ConfigurationComponent } from './components/configuration/configuration.component';
import { UserSessionComponent } from './components/usersession/usersession.component';
import { AuthGuard } from './services/auth.service';

const routes: Routes = [
  {
    path: 'session',
    component: UserSessionComponent,
    children: [
      {
        path: 'orchestrations', component: OrchestrationComponent, canActivate: [AuthGuard]
      },
      {
        path: 'configurations', component: ConfigurationComponent, canActivate: [AuthGuard]
      },
      {
        path: '',
        redirectTo: 'organizations',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: 'default', component: DefaultComponent
  },
  {
    path: '**', redirectTo: 'default'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
