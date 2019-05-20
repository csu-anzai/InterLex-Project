import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {TreeNode} from 'primeng/api';
import {HttpService} from '../../core/services/http.service';

@Component({
  selector: 'app-eurovoc-tree',
  template: `
    <div class="ui-inputgroup">
      <input type="text" pInputText placeholder="Keyword" [(ngModel)]="filterText" (keyup.enter)="filterTree()">
      <button pButton type="button" icon="pi pi-times" class="ui-button-danger" (click)="clearFilter()"></button>
      <button pButton type="button" label="Search" (click)="filterTree()"></button>
    </div>
    <p-tree [value]="euroTree" [propagateSelectionDown]="false" [propagateSelectionUp]="false" [loading]="loading"
            (onNodeExpand)="loadNodeContent($event)"
            selectionMode="checkbox" [(selection)]="selected" [style]="{'width':'100%'}"></p-tree>
    <button pButton (click)="done()" label="Done"></button>
    <button pButton (click)="clear()" label="Clear Selection"></button>`,
  styles: []
})
export class EurovocTreeComponent implements OnInit {

  selected: TreeNode[] = [];
  selectedClone: TreeNode[] = [];
  codesToFilterBy: string[];
  filterText: string;

  @Input() set selectedSet(value: any[]) {
    if (!!!value || value.every(x => x.selectable === true)) {// case with real TreeNode data
      this.selected = value || [];
      this.codesToFilterBy = [];
    } else {  // case with data from api json - only names and codes
      this.codesToFilterBy = value.map(x => x.code);
      if (this.euroTreeFull) {
        this.selected = [];
        this.selectNodesByFilter(this.euroTreeFull);  // needed when selection is cleared but tree is not saved
      }
    }
  }

  euroTree: TreeNode[];
  euroTreeFull: TreeNode[];
  loading: boolean;

  @Output() selectionConfirmed = new EventEmitter<TreeNode[]>();

  constructor(private http: HttpService) {
  }

  ngOnInit() {
    this.loading = true;
    this.http.get('case/GetWholeTree').subscribe((x: any[]) => {  // GetWholeTree for filtering
      this.euroTree = x;
      this.euroTreeFull = x;
      this.loading = false;
      if (this.codesToFilterBy) {
        this.selectNodesByFilter(this.euroTreeFull);
      }
    });
  }

  loadNodeContent(event) {
    return;
    if (event.node && !event.node.children) {
      this.loading = true;
      const node = event.node;
      this.http.get('case/GetTreeData/' + node.id).subscribe((x: any[]) => {
        node.children = x;
        this.loading = false;
      });
    }
  }

  filterTree() {
    if (!this.filterText) {
      return;
    }
    if (!this.selectedClone.length) {
      this.selectedClone = [...this.selected];
    }
    this.selected = [];
    this.codesToFilterBy = this.selectedClone.map(x => x.data);
    this.euroTree = this.filterData(this.euroTreeFull, x => x.label.toLowerCase().includes(this.filterText.toLowerCase()));
    // delete below if shit happens
  }

  filterData(data: TreeNode[], predicate) {

    // if no data is sent in, return null, otherwise transform the data
    return !!!data ? null : data.reduce((list, entry) => {
      let clone: TreeNode = null;
      if (entry.children.length === 0 && predicate(entry)) {
        // if the object matches the filter, clone it as it is
        clone = {...entry};
      } else {
        // if the object has children, filter the list of children
        const children = this.filterData(entry.children, predicate);
        if (children.length > 0) {
          // if any of the children matches, clone the parent object, overwrite
          // the children list with the filtered list
          clone = {...entry, children: children};
        } else if (predicate(entry)) {
          // if parent matches but none of the children do add only parent
          clone = {...entry, children: []};
        }
      }
      // if there's a cloned object, push it to the output list
      if (clone) {
        clone.expanded = true;
        list.push(clone);
        if (this.codesToFilterBy.includes(clone.data)) {
          this.selected.push(clone);
        }
      }
      return list;
    }, []);
  }

  clearFilter() {
    if (this.selectedClone.length) {
      this.selected = [...this.selectedClone];
    }
    this.euroTree = this.euroTreeFull;
    this.filterText = '';
  }

  done() {
    // sanitize all selected data so that it comes from full tree and not filtered!
    const hash = new Set<string>();
    if (this.selectedClone.length) {
      this.selectedClone.forEach(x => hash.add(x.data));
    }
    if (this.selected.length) {
      this.selected.forEach(x => hash.add(x.data));
    }
    this.codesToFilterBy = Array.from(hash);
    this.selected = [];
    this.selectNodesByFilter(this.euroTreeFull);
    this.euroTree = this.euroTreeFull;
    this.filterText = '';
    this.selectionConfirmed.emit(this.selected);
  }

  clear() {
    this.selected.length = 0;
  }

  private selectNodesByFilter(nodes: TreeNode[]) {
    let hasSelectedChildren = false;
    for (const node of nodes) {
      if (this.codesToFilterBy.includes(node.data)) {
        if (!this.selected.some(x => x.data === node.data)) {
          this.selected.push(node);
        }
        hasSelectedChildren = true;
      }
      if (node.children) {
        const res = this.selectNodesByFilter(node.children);
        if (res) {
          node.expanded = true;
          hasSelectedChildren = hasSelectedChildren || res;
        } else {
          node.expanded = false;
        }
      }
    }
    return hasSelectedChildren;
  }
}
