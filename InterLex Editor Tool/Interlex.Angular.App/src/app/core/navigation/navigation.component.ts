import {Component, OnDestroy, OnInit} from '@angular/core';
import {MenuItem} from 'primeng/api';
import {AuthService} from '../services/auth.service';
import {Subscription} from 'rxjs';
import {Router} from '@angular/router';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.css']
})
export class NavigationComponent implements OnInit, OnDestroy {
  items: MenuItem[];
  loggedInInfo: Subscription;
  buttonItems: MenuItem[];

  constructor(private auth: AuthService, private router: Router) {
  }

  username: string;
  loggedIn: boolean;

  ngOnInit() {
    this.items = [
      // {
      //   label: 'Case editor',
      //   icon: 'pi pi-fw pi-pencil',
      //   routerLink: ['caseeditor']
      // },
      {
        label: 'Case list',
        icon: 'pi pi-fw pi-file ',
        routerLink: ['caselist']
      },
      // {
      //   label: 'Home',
      //   icon: 'pi pi-fw pi-cog',
      //   routerLink: ['home'],
      // }
    ];
    this.buttonItems = [
      {
        label: 'Change Password', icon: 'pi pi-key', command: () => {
          this.changePass();
        }
      }
    ];
    this.loggedInInfo = this.auth.getLoggedInStatus().subscribe(flag => {
      this.items = this.items.filter(x => x.label !== 'Admin');
      if (flag) {
        this.username = this.auth.getUsername();
        this.loggedIn = true;
        if (this.auth.isAdmin()) {
          this.items.push({label: 'Admin', routerLink: ['admin']});
        }
        this.addHelpButton();
      } else {
        this.username = 'Anonymous';
        this.loggedIn = false;
      }
    });
  }

  changePass() {
    this.router.navigate(['edit']);
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  ngOnDestroy(): void {
    this.loggedInInfo.unsubscribe();
  }

  private addHelpButton() {
    if (!this.items.find(x => x.label === 'Help')) {
      this.items.push({label: 'Help', routerLink: ['help']});
    }

    const indexAdmin = this.items.findIndex(x => x.label === 'Admin');  // subscription messes link order, so fixing here
    const indexHelp = this.items.findIndex(x => x.label === 'Help');
    if (indexHelp < indexAdmin) {
      const temp = this.items[indexAdmin];
      this.items[indexAdmin] = this.items[indexHelp];
      this.items[indexHelp] = temp;
    }
  }
}
