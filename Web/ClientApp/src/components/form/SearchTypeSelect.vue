<template>
  <v-select
    class="mb-2"
    :value="value"
    @input="x => $emit('input', x)"
    :items="sorts"
    :label="value"
    :dense="dense"
    :single-line="singleLine"
    :hide-details="hideDetails"
  >
  </v-select>
</template>

<script lang="ts">
import { defineComponent } from '@vue/composition-api';

export default defineComponent({
  name: 'SearchTypeSelect',
  props: {
    value: String,
    label: String,
    persistentPlaceholder: Boolean,
    dense: Boolean,
    singleLine: Boolean,
    hideDetails: Boolean,
  },
  setup() {
    return {
      sorts: [
        { text: 'Threads', value: '' },
        { text: 'Posts', value: 'post' },
      ],
    };
  },
});
</script>
